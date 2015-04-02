//
//  HomeViewController.m
//  Azure Activity Logger
//

#import "HomeViewController.h"
#import "AzureConnector.h"
#import "FetchedResultsDataSource.h"
#import "DataAccessor.h"
#import "Contact.h"
#import "ColorsAndFonts.h"
#import "NSString+StringFormatting.h"

#import "SettingsViewController.h"
#import "ObjectDetailsViewController.h"

#import "ADAuthenticationError.h"

NSString *const SubtitleTableViewCellIdentifier = @"SubtitleTableViewCellIdentifier";

@interface HomeViewController () <UISearchBarDelegate, UITableViewDelegate>

@property (weak, nonatomic) IBOutlet UISearchBar *theSearchBar;
@property (weak, nonatomic) IBOutlet UITableView *resultsTableView;
@property (weak, nonatomic) IBOutlet UIImageView *loadingIndicator;
@property (weak, nonatomic) IBOutlet UIButton *syncButton;
@property (weak, nonatomic) IBOutlet UIActivityIndicatorView *syncActivityIndicator;
@property (weak, nonatomic) IBOutlet UIView *standardLabelsContainerView;
@property (weak, nonatomic) IBOutlet UILabel *lastSyncLabel;
@property (weak, nonatomic) IBOutlet UILabel *syncCountLabel;
@property (weak, nonatomic) IBOutlet UILabel *syncingLabel;
@property (weak, nonatomic) IBOutlet UIView *syncingLabelContainerView;

@property (strong, nonatomic) UIView *syncingOverlayView;

@property (nonatomic, strong) FetchedResultsDataSource *contactsDataSource;

@end

@implementation HomeViewController

- (void)viewDidLoad {
    [super viewDidLoad];
    // Do any additional setup after loading the view, typically from a nib.

    self.title = @"Activity Logger";

    self.resultsTableView.tableFooterView = [[UIView alloc] initWithFrame:CGRectZero];
    [self.resultsTableView registerNib:[UINib nibWithNibName:@"SubtitleTableViewCell" bundle:nil] forCellReuseIdentifier:SubtitleTableViewCellIdentifier];

    [self configureContactsResultController:[[DataAccessor sharedAccessor] contactsFetchedResultsController]];
    
    self.resultsTableView.delegate = self;

    self.theSearchBar.delegate = self;

    self.navigationItem.backBarButtonItem = [[UIBarButtonItem alloc] initWithTitle:@"" style:UIBarButtonItemStylePlain target:nil action:nil];
    self.navigationItem.rightBarButtonItem = [[UIBarButtonItem alloc] initWithImage:[UIImage imageNamed:@"icon-settings"] landscapeImagePhone:[UIImage imageNamed:@"icon-settings"] style:UIBarButtonItemStylePlain target:self action:@selector(settingsClick:)];

    self.syncActivityIndicator.hidden = YES;
    self.syncingLabelContainerView.hidden = YES;

    self.standardLabelsContainerView.hidden = NO;

}

- (void)viewWillAppear:(BOOL)animated {
    [super viewWillAppear:animated];

    [self updateSyncLabels];
    [self setupNotifications];
}

- (void)viewWillDisappear:(BOOL)animated {
    [super viewWillDisappear:animated];

    [self teardownNotifications];
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

+ (NSDateFormatter *)syncDateFormatter {
    static NSDateFormatter *formatter;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        formatter = [[NSDateFormatter alloc] init];
        formatter.dateFormat = @"M/d/yyyy  h:mm a";
    });
    return formatter;
}

- (void)updateSyncLabels {
    self.lastSyncLabel.text = [NSString stringWithFormat:@"Last Sync: %@", [NSString dashesForEmpty:[[HomeViewController syncDateFormatter] stringFromDate:[[AzureConnector sharedConnector] lastSyncDate]]]];
    NSUInteger pendingSyncs = [[AzureConnector sharedConnector] pendingSyncCount];
    NSString *itemText;
    if (pendingSyncs == 1) {
        itemText = @"item";
    } else {
        itemText = @"items";
    }
    self.syncCountLabel.text = [NSString stringWithFormat:@"%@ %@ to sync", @(pendingSyncs), itemText];
}

- (void)configureContactsResultController:(NSFetchedResultsController *)controller {
    self.contactsDataSource = [[FetchedResultsDataSource alloc] initWithFetchedResultsController:controller];

    __weak typeof(self) weakSelf = self;
    self.contactsDataSource.cellConfigureBlock = ^UITableViewCell *(UITableView *tableView, Contact *theContact, NSIndexPath *indexPath) {
        UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:SubtitleTableViewCellIdentifier forIndexPath:indexPath];
        cell.textLabel.text = theContact.resultLine1;
        cell.detailTextLabel.text = theContact.resultLine2;
        cell.imageView.image = [UIImage imageNamed:@"icon-contact"];
        cell.selectedBackgroundView.backgroundColor = BLUE_HIGHLIGHT;
        cell.textLabel.textColor = LIGHT_BLUE;
        cell.detailTextLabel.textColor = MEDIUM_GREY;
        return cell;
    };
    self.contactsDataSource.resultsDidChangeBlock = ^() {
        dispatch_async(dispatch_get_main_queue(), ^{
            __strong typeof(self) strongSelf = weakSelf;
            [strongSelf.resultsTableView reloadData];
        });
    };
    
    self.resultsTableView.dataSource = self.contactsDataSource;
    [self.resultsTableView reloadData];
}

#pragma mark - Notification methods

// Need to listen for completed syncs so we can update the display.
- (void)setupNotifications {
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(syncCompleted:) name:AzureConnectorSyncCompleted object:nil];
}

- (void)teardownNotifications {
    [[NSNotificationCenter defaultCenter] removeObserver:self];
}

- (void)syncCompleted:(NSNotification *)theNotification {
    dispatch_async(dispatch_get_main_queue(), ^{
        CATransition *animation = [CATransition animation];
        animation.timingFunction = [CAMediaTimingFunction functionWithName:kCAMediaTimingFunctionEaseInEaseOut];
        animation.type = kCATransitionFade;
        animation.duration = 0.75;
        [self.syncingLabel.layer addAnimation:animation forKey:@"kCATransitionFade"];
        
        NSString *syncResultText;
        if ([theNotification.userInfo[AzureConnectorSyncSuccessKey] boolValue]) {
            syncResultText = @"Sync complete";
        } else {
            syncResultText = @"Sync failed";
        }
        
        // This will fade:
        self.syncingLabel.text = syncResultText;
        
        [UIView animateWithDuration:0.5 animations:^{
            self.syncingOverlayView.alpha = 0.0;
        } completion:^(BOOL finished) {
            [self.syncingOverlayView removeFromSuperview];
            self.syncingOverlayView = nil;
            
            [self.syncActivityIndicator stopAnimating];
            self.syncActivityIndicator.hidden = YES;
        }];
    });
    
    dispatch_after(dispatch_time(DISPATCH_TIME_NOW, (int64_t)(5.0 * NSEC_PER_SEC)), dispatch_get_main_queue(), ^{
        self.standardLabelsContainerView.hidden = NO;
        self.standardLabelsContainerView.alpha = 0.0;
        self.syncButton.hidden = NO;
        self.syncButton.alpha = 0.0;
        
        [UIView animateWithDuration:0.5 animations:^{
            self.syncingLabelContainerView.alpha = 0.0;
            self.standardLabelsContainerView.alpha = 1.0;
            self.syncButton.alpha = 1.0;
        } completion:^(BOOL finished) {
            self.syncingLabelContainerView.hidden = YES;
            
            self.syncButton.enabled = YES;

            [self updateSyncLabels];
        }];
    });
}

#pragma mark - Action methods

- (void)settingsClick:(id)sender {
    SettingsViewController *settingsView = [[SettingsViewController alloc] initWithNibName:@"SettingsView" bundle:nil];
    [self.navigationController pushViewController:settingsView animated:YES];
}

- (IBAction)syncButtonTapped:(id)sender {
    self.syncingLabel.text = @"Sync in progress ...";
    
    // Setup the gray overlay.
    self.syncingOverlayView = [[UIView alloc] initWithFrame:self.navigationController.view.frame];
    CGRect shadedViewFrame = self.navigationController.view.frame;
    shadedViewFrame.size.height = shadedViewFrame.size.height - 44.0;
    UIView *shadedView = [[UIView alloc] initWithFrame:shadedViewFrame];
    shadedView.backgroundColor = [[UIColor blackColor] colorWithAlphaComponent:0.60];
    [self.syncingOverlayView addSubview:shadedView];
    self.syncingOverlayView.userInteractionEnabled = YES;
    [self.navigationController.view addSubview:self.syncingOverlayView];

    [self.syncActivityIndicator startAnimating];
    self.syncActivityIndicator.hidden = NO;

    self.syncingLabelContainerView.alpha = 1.0;
    self.syncingLabelContainerView.hidden = NO;
    self.standardLabelsContainerView.hidden = YES;

    self.syncButton.enabled = NO;
    self.syncButton.hidden = YES;

    __weak typeof(self) weakSelf = self;
    [[AzureConnector sharedConnector] syncWithCompletion:^(NSError *error) {
        __strong typeof(weakSelf) strongSelf = weakSelf;

        // If there was an error, display it to the user in a useful manner.
        if (error) {
            NSString *detailsString = @"";
            if ([error isKindOfClass:[ADAuthenticationError class]]) {
                detailsString = ((ADAuthenticationError *)error).errorDetails;
            }
            UIAlertController *alert = [UIAlertController alertControllerWithTitle:@"Sync Error" message:[NSString stringWithFormat:@"There was an error syncing, please try again later.\n%@", detailsString] preferredStyle:UIAlertControllerStyleAlert];
            UIAlertAction *okay = [UIAlertAction actionWithTitle:@"Okay" style:UIAlertActionStyleDefault handler:nil];
            [alert addAction:okay];

            [strongSelf presentViewController:alert animated:YES completion:nil];
        }
    }];
}

#pragma mark - UITableViewDelegate methods

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath {
    Contact *theContact = [self.contactsDataSource itemAtIndexPath:indexPath];
    
    ObjectDetailsViewController *detailsView = [[ObjectDetailsViewController alloc] initWithNibName:@"ObjectDetailsView" bundle:nil];
    detailsView.displayObject = theContact;

    [self.navigationController pushViewController:detailsView animated:YES];
}

#pragma mark - UISearchBarDelegate methods

- (void)searchBarTextDidBeginEditing:(UISearchBar *)searchBar {
    [searchBar setShowsCancelButton:YES animated:YES];
}

- (void)searchBarTextDidEndEditing:(UISearchBar *)searchBar {
    [self configureContactsResultController:[[DataAccessor sharedAccessor] contactsFetchedResultsController]];
}

- (void)searchBar:(UISearchBar *)searchBar textDidChange:(NSString *)searchText {
    if (searchText.length == 0) {
        [self configureContactsResultController:[[DataAccessor sharedAccessor] contactsFetchedResultsController]];
        return;
    }

    NSPredicate *firstNamePredicate = [NSPredicate predicateWithFormat:@"firstName CONTAINS[cd] %@", searchText];
    NSPredicate *lastNamePredicate = [NSPredicate predicateWithFormat:@"lastName CONTAINS[cd] %@", searchText];
    NSPredicate *jobTitlePredicate = [NSPredicate predicateWithFormat:@"jobTitle CONTAINS[cd] %@", searchText];
    NSPredicate *textSearchPredicate = [NSCompoundPredicate orPredicateWithSubpredicates:@[firstNamePredicate, lastNamePredicate, jobTitlePredicate]];

    [self configureContactsResultController:[[DataAccessor sharedAccessor] fetchedResultsControllerForObject:@"Contact"
                                                                                               withPredicate:textSearchPredicate
                                                                                             sortDescriptors:@[ [NSSortDescriptor sortDescriptorWithKey:@"lastName" ascending:YES] ]]];
}

- (void)searchBarCancelButtonClicked:(UISearchBar *)searchBar {
    searchBar.text = @"";
    [self configureContactsResultController:[[DataAccessor sharedAccessor] contactsFetchedResultsController]];
    [searchBar setShowsCancelButton:NO animated:YES];
    [searchBar resignFirstResponder];
}

@end
