#import "ObjectDetailsViewController.h"
#import "NSString+StringFormatting.h"
#import "ColorsAndFonts.h"
#import "NewActivityViewController.h"
#import "DataAccessor.h"
#import "ActivityDisplayable.h"

@interface ObjectDetailsViewController () <UITableViewDataSource, UITableViewDelegate> {
    IBOutlet UIView *detailsView;
    IBOutlet UILabel *mainLabel;
    IBOutlet UILabel *secondaryLabel;
    IBOutlet UILabel *thirdLabel;
    IBOutlet UILabel *addressLabel;
    IBOutlet UILabel *phoneLabel;
    IBOutlet UILabel *emailLabel;

    IBOutlet UITableView *activitiesList;
    IBOutlet UIImageView *loadingIndicator;
}

@property (nonatomic, strong) NSArray *relatedActivities;

@end

@implementation ObjectDetailsViewController

@synthesize displayObject;

- (id)initWithNibName:(NSString *)nibNameOrNil bundle:(NSBundle *)nibBundleOrNil {
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self) {

    }
    return self;
}

- (void)viewDidLoad {
    [super viewDidLoad];
    // Do any additional setup after loading the view.

    self.title = @"Contact";

    detailsView.backgroundColor = VERY_LIGHT_GREY;
    mainLabel.textColor = LIGHT_BLUE;
    secondaryLabel.textColor = MEDIUM_GREY;
    thirdLabel.textColor = MEDIUM_GREY;
    addressLabel.textColor = DARK_GREY;
    phoneLabel.textColor = DARK_GREY;
    emailLabel.textColor = DARK_GREY;

    self.navigationItem.backBarButtonItem = [[UIBarButtonItem alloc] initWithTitle:@"" style:UIBarButtonItemStylePlain target:nil action:nil];

    activitiesList.dataSource = self;
    activitiesList.delegate = self;
    activitiesList.tableFooterView = [[UIView alloc] initWithFrame:CGRectZero];
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

- (void)viewWillAppear:(BOOL)animated {
    [super viewWillAppear:animated];
    [self refreshObject];
}

- (void)refreshObject {
    mainLabel.text = [NSString dashesForEmpty:displayObject.detailLine1];
    secondaryLabel.text = [NSString dashesForEmpty:displayObject.detailLine2];
    thirdLabel.text = [NSString dashesForEmpty:displayObject.detailLine3];
    addressLabel.text = [NSString dashesForEmpty:displayObject.addressInfo];
    phoneLabel.text = [NSString dashesForEmpty:displayObject.mainPhone];
    emailLabel.text = [NSString dashesForEmpty:displayObject.mainEmail];

    self.relatedActivities = [[DataAccessor sharedAccessor] getActivitiesSatisfyingPredicate:[NSPredicate predicateWithFormat:@"regardingObjectId = %@", displayObject.id]];
    self.relatedActivities = [self.relatedActivities sortedArrayUsingDescriptors:@[ [NSSortDescriptor sortDescriptorWithKey:@"actualEnd" ascending:NO] ]];

    [activitiesList setContentOffset:CGPointMake(0, 0)];
}


- (IBAction)actionClick:(id)sender {
    NewActivityViewController *activityView = [[NewActivityViewController alloc] initWithNibName:@"NewActivityView" bundle:nil];
    activityView.displayObject = self.displayObject;

    UIButton *activityButton = (UIButton *) sender;

    switch (activityButton.tag) {
        case 10:
            activityView.activityType = @"Check In";
            break;
        case 11:
            activityView.activityType = @"Note";
            break;
        case 12:
            activityView.activityType = @"Follow Up";
            break;
        case 13:
            activityView.activityType = @"Phone Call";
            break;
    }

    [self.navigationController pushViewController:activityView animated:YES];
}

- (IBAction)addressClick:(id)sender {
    if ([displayObject.addressInfo length] == 0) {
        return;
    }

    NSString *addressString = [addressLabel.text stringByAddingPercentEscapesUsingEncoding:NSUTF8StringEncoding];

    NSURL *mapsURL = [NSURL URLWithString:[NSString stringWithFormat:@"http://maps.apple.com?q=%@", addressString]];

    [[UIApplication sharedApplication] openURL:mapsURL];
}

- (IBAction)phoneClick:(id)sender {
    if ([displayObject.mainPhone length] == 0) {
        return;
    }

    NSString *cleanedString = [[displayObject.mainPhone componentsSeparatedByCharactersInSet:[[NSCharacterSet characterSetWithCharactersInString:@"0123456789-+()"] invertedSet]] componentsJoinedByString:@""];
    NSString *escapedPhoneNumber = [cleanedString stringByAddingPercentEscapesUsingEncoding:NSUTF8StringEncoding];

    NSURL *phoneURL = [NSURL URLWithString:[NSString stringWithFormat:@"telprompt:%@", escapedPhoneNumber]];

    [[UIApplication sharedApplication] openURL:phoneURL];
}

- (IBAction)emailClick:(id)sender {
    if ([displayObject.mainEmail length] == 0) {
        return;
    }

    NSURL *emailURL = [NSURL URLWithString:[NSString stringWithFormat:@"mailto:%@", displayObject.mainEmail]];

    [[UIApplication sharedApplication] openURL:emailURL];
}


#pragma mark - UITableView DataSource

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section {
    return self.relatedActivities.count;
}

- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath {
    static NSString *CellIdentifier = @"ResultsCell";

    NSDateFormatter *dateFormat = [[NSDateFormatter alloc] init];
    dateFormat.dateFormat = @"MMM d, yyyy";

    UITableViewCell *retVal = [tableView dequeueReusableCellWithIdentifier:CellIdentifier];
    if (retVal == nil) {
        retVal = [[UITableViewCell alloc] initWithStyle:UITableViewCellStyleSubtitle reuseIdentifier:CellIdentifier];
    }

    id <ActivityDisplayable> curActivity = self.relatedActivities[indexPath.row];

    retVal.textLabel.text = curActivity.subject;
    retVal.textLabel.textColor = DARK_PURPLE;

    retVal.detailTextLabel.text = [dateFormat stringFromDate:curActivity.actualEnd];
    retVal.detailTextLabel.textColor = MEDIUM_GREY;

    retVal.imageView.image = [curActivity activityImage];

    return retVal;
}

- (NSString *)tableView:(UITableView *)tableView titleForHeaderInSection:(NSInteger)section {
    return @"RECENTLY COMPLETED ACTIVITIES";
}

#pragma mark - UITableView Delegate

- (NSIndexPath *)tableView:(UITableView *)tableView willSelectRowAtIndexPath:(NSIndexPath *)indexPath {
    return nil;
}

- (CGFloat)tableView:(UITableView *)tableView heightForHeaderInSection:(NSInteger)section {
    return 26.0;
}

- (UIView *)tableView:(UITableView *)tableView viewForHeaderInSection:(NSInteger)section {
    UIView *header = [[UIView alloc] initWithFrame:CGRectMake(0, 0, tableView.frame.size.width, [tableView.delegate tableView:tableView heightForHeaderInSection:section])];
    header.backgroundColor = [UIColor whiteColor];

    UILabel *textLabel = [[UILabel alloc] init];
    textLabel.text = [tableView.dataSource tableView:tableView titleForHeaderInSection:section];
    textLabel.textColor = DARK_GREY;
    textLabel.font = [textLabel.font fontWithSize:14.0];
    [textLabel sizeToFit];
    textLabel.frame = CGRectMake(20,
        (header.frame.size.height - textLabel.frame.size.height) / 2.0,
        textLabel.frame.size.width,
        textLabel.frame.size.height);
    [header addSubview:textLabel];

    return header;
}

@end
