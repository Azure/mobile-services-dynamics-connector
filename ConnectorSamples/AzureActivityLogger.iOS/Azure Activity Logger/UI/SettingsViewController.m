//
//  SettingsViewController.m
//  Azure Activity Logger
//

#import "SettingsViewController.h"
#import "ColorsAndFonts.h"

#import "AzureConnector.h"
#import "ADAuthenticationError.h"

@interface SettingsViewController ()

@property (weak, nonatomic) IBOutlet UIButton *loginButton;
@property (weak, nonatomic) IBOutlet UIButton *logoutButton;
@property (weak, nonatomic) IBOutlet UILabel *editableInfoLabel;
@property (weak, nonatomic) IBOutlet UIButton *resetToDefaultsButton;
@property (strong, nonatomic) IBOutletCollection(UIButton) NSArray *buttons;

@property (weak, nonatomic) IBOutlet UITextField *applicationURLTextField;
@property (weak, nonatomic) IBOutlet UITextField *clientIDTextField;
@property (strong, nonatomic) IBOutletCollection(UITextField) NSArray *textFields;

@property (strong, nonatomic) IBOutletCollection(UIView) NSArray *containerViews;

@end

@implementation SettingsViewController

- (id)initWithNibName:(NSString *)nibNameOrNil bundle:(NSBundle *)nibBundleOrNil {
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self) {

    }
    return self;
}

- (void)viewDidLoad {
    [super viewDidLoad];

    self.title = @"Settings";

    [self updateTextFields];
    [self updateViewForLoggedIn:[[AzureConnector sharedConnector] isLoggedIn]];

    for (UIView *view in self.containerViews) {
        view.layer.borderColor = LINE_COLOR.CGColor;
        view.layer.borderWidth = 1.0;
    }

    for (UIButton *button in self.buttons) {
        button.layer.borderWidth = 1.0;
        button.layer.borderColor = [UIColor colorWithRed:0 green:0.6 blue:0.98 alpha:1].CGColor;
        button.layer.cornerRadius = 7.0;
    }
}

- (void)viewWillAppear:(BOOL)animated {
    [super viewWillAppear:animated];
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

- (void)updateViewForLoggedIn:(BOOL)loggedIn {
    for (UITextField *textField in self.textFields) {
        textField.enabled = !loggedIn;
    }

    self.loginButton.enabled = !loggedIn;
    self.loginButton.hidden = loggedIn;
    self.logoutButton.enabled = loggedIn;
    self.logoutButton.hidden = !loggedIn;
    self.editableInfoLabel.hidden = !loggedIn;
    
    self.resetToDefaultsButton.enabled = !loggedIn;
}

- (void)updateTextFields {
    self.applicationURLTextField.text = [AzureConnector sharedConnector].applicationURL;
    self.clientIDTextField.text = [AzureConnector sharedConnector].clientID;
}

#pragma mark - Action methods

- (IBAction)loginButtonTapped:(id)sender {
    [AzureConnector sharedConnector].applicationURL = self.applicationURLTextField.text;
    [AzureConnector sharedConnector].clientID = self.clientIDTextField.text;

    __weak typeof(self) weakSelf = self;
    [[AzureConnector sharedConnector] loginWithController:self completion:^(MSUser *user, NSError *error) {
        __strong typeof(self) strongSelf = weakSelf;

        if (error) {
            NSString *detailsString = @"";
            if ([error isKindOfClass:[ADAuthenticationError class]]) {
                detailsString = ((ADAuthenticationError *)error).errorDetails;
            }
            UIAlertController *alert = [UIAlertController alertControllerWithTitle:@"Sync Error" message:[NSString stringWithFormat:@"There was an error syncing, please try again later.\n%@", detailsString] preferredStyle:UIAlertControllerStyleAlert];
            UIAlertAction *okay = [UIAlertAction actionWithTitle:@"Okay" style:UIAlertActionStyleDefault handler:nil];
            [alert addAction:okay];
            
            [strongSelf presentViewController:alert animated:YES completion:nil];
            return;
        }

        dispatch_async(dispatch_get_main_queue(), ^{
            [strongSelf updateViewForLoggedIn:YES];
            [strongSelf.navigationController popViewControllerAnimated:YES];
            // keep the logout button disabled until sync is complete
            // strongSelf.logoutButton.enabled = NO;
        });
    }];
}

- (IBAction)logoutButtonTapped:(id)sender {
    if ([[AzureConnector sharedConnector] pendingSyncCount] > 0) {
        UIAlertController *confirmAlert = [UIAlertController alertControllerWithTitle:@"Confirm Logout" message:@"You have pending changes that will be lost if you log out. Continue?" preferredStyle:UIAlertControllerStyleAlert];
        UIAlertAction *continueAction = [UIAlertAction actionWithTitle:@"Continue" style:UIAlertActionStyleDefault handler:^(UIAlertAction *action) {
            [self performLogout];
        }];
        UIAlertAction *cancelAction = [UIAlertAction actionWithTitle:@"Cancel" style:UIAlertActionStyleCancel handler:nil];
        [confirmAlert addAction:continueAction];
        [confirmAlert addAction:cancelAction];
        
        [self presentViewController:confirmAlert animated:YES completion:nil];
        return;
    }
    [self performLogout];
}

- (void)performLogout {
    [[AzureConnector sharedConnector] logout];
    [self updateViewForLoggedIn:[[AzureConnector sharedConnector] isLoggedIn]];
}

- (IBAction)resetToDefaultsButtonTapped:(id)sender {
    [AzureConnector sharedConnector].applicationURL = nil;
    [AzureConnector sharedConnector].clientID = nil;
    
    [self updateTextFields];
}

@end
