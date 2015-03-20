//
//  NewActivityViewController.m
//  Azure Activity Logger
//

#import "NewActivityViewController.h"
#import "ColorsAndFonts.h"
#import "AzureConnector.h"

@interface NewActivityViewController () {
    IBOutlet UIImageView *loadingIndicator;
    IBOutlet UIView *detailsView;
    IBOutlet UILabel *mainLabel;
    IBOutlet UILabel *secondaryLabel;
    IBOutlet UILabel *thirdLabel;

    IBOutlet UIScrollView *scroller;
    IBOutlet UIView *scrollerBackground;

    IBOutlet UITextField *subjectField;
    IBOutlet UITextField *dateField;
    IBOutlet UITextView *notesView;

    IBOutletCollection(UILabel) NSArray *fieldLabels;

    NSDate *activityDate;
    NSDateFormatter *dateFormat;
}
@end

@implementation NewActivityViewController

@synthesize displayObject, activityType;

- (id)initWithNibName:(NSString *)nibNameOrNil bundle:(NSBundle *)nibBundleOrNil {
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self) {
    }
    return self;
}

- (void)viewDidLoad {
    [super viewDidLoad];
    // Do any additional setup after loading the view.

    for (UILabel *label in fieldLabels) {
        label.textColor = DARK_GREY;
    }

    detailsView.backgroundColor = VERY_LIGHT_GREY;
    scrollerBackground.backgroundColor = LINE_COLOR;

    mainLabel.text = displayObject.detailLine1;
    mainLabel.textColor = LIGHT_BLUE;

    secondaryLabel.text = displayObject.detailLine2;
    secondaryLabel.textColor = MEDIUM_GREY;

    thirdLabel.text = displayObject.detailLine3;
    thirdLabel.textColor = MEDIUM_GREY;

    activityDate = [NSDate date];
    dateFormat = [[NSDateFormatter alloc] init];
    dateFormat.dateFormat = @"M/d/yyyy";

    dateField.text = [dateFormat stringFromDate:activityDate];

    UIDatePicker *datePicker = [[UIDatePicker alloc] initWithFrame:CGRectZero];
    datePicker.date = activityDate;
    datePicker.datePickerMode = UIDatePickerModeDate;
    [datePicker addTarget:self action:@selector(dateChange:) forControlEvents:UIControlEventValueChanged];

    dateField.inputView = datePicker;

    self.title = self.activityType;

    subjectField.text = [NSString stringWithFormat:@"%@ with %@ on %@", self.activityType, self.displayObject.resultLine1, [dateFormat stringFromDate:activityDate]];

    self.navigationItem.rightBarButtonItem = [[UIBarButtonItem alloc] initWithTitle:@"Submit" style:UIBarButtonItemStylePlain target:self action:@selector(submitClick:)];
    self.navigationItem.rightBarButtonItem.tintColor = LIGHT_BLUE;
}

- (void)viewDidAppear:(BOOL)animated {
    [super viewDidAppear:animated];

    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(keyboardShow:) name:UIKeyboardWillShowNotification object:nil];

    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(keyboardWillHide:) name:UIKeyboardWillHideNotification object:nil];
}

- (void)viewWillDisappear:(BOOL)animated {
    [super viewWillDisappear:animated];

    [[NSNotificationCenter defaultCenter] removeObserver:self];
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

- (void)dateChange:(id)sender {
    UIDatePicker *picker = (UIDatePicker *) sender;

    if ([subjectField.text isEqualToString:[NSString stringWithFormat:@"%@ with %@ on %@", self.activityType, self.displayObject.resultLine1, [dateFormat stringFromDate:activityDate]]]) {
        activityDate = picker.date;
        dateField.text = [dateFormat stringFromDate:activityDate];
        subjectField.text = [NSString stringWithFormat:@"%@ with %@ on %@", self.activityType, self.displayObject.resultLine1, [dateFormat stringFromDate:activityDate]];
    }
    else {
        activityDate = picker.date;
        dateField.text = [dateFormat stringFromDate:activityDate];
    }
}

- (void)submitClick:(id)sender {
    if ([subjectField.text length] == 0) {
        UIAlertView *error = [[UIAlertView alloc] initWithTitle:@"Subject Required" message:@"A subject is required." delegate:nil cancelButtonTitle:@"Ok" otherButtonTitles:nil];
        [error show];
        return;
    }
    self.navigationItem.rightBarButtonItem.enabled = NO;
    [self.navigationItem setHidesBackButton:YES animated:YES];
    [self.view endEditing:YES];

    NSMutableDictionary *task = [NSMutableDictionary dictionaryWithDictionary:@{
        @"actualEnd"         : activityDate,
        @"subject"           : subjectField.text,
        @"regardingObjectId" : displayObject.id
    }];
    if (notesView.text) {
        task[@"detail"] = notesView.text;
    }

    // It is necessary to use an async method to insert the task because MWS
    // does not guarantee that the insert will complete by the time the method
    // returns.
    [[AzureConnector sharedConnector] insertTask:[task copy] completion:^(NSDictionary *item, NSError *error) {
        if (error) {
            NSLog(@"Error inserting task : %@\n%@", error.localizedDescription, error.localizedFailureReason);
            return;
        }
        NSLog(@"Inserted item : %@", item);
        dispatch_async(dispatch_get_main_queue(), ^{
            [self.navigationController popViewControllerAnimated:YES];
        });
    }];
}

#pragma mark - Keyboard Notifications

- (void)keyboardShow:(NSNotification *)notification {
    CGRect keyboardFrame = [[notification.userInfo objectForKey:UIKeyboardFrameEndUserInfoKey] CGRectValue];
    CGSize contentSize = CGSizeMake(scroller.frame.size.width, scroller.frame.size.height + keyboardFrame.size.height);
    [scroller setContentSize:contentSize];
    scroller.scrollEnabled = YES;

}

- (void)keyboardWillHide:(NSNotification *)notification {
    scroller.scrollEnabled = NO;
    [scroller setContentOffset:CGPointMake(0, 0) animated:YES];
}

#pragma mark - UITextView Delegate

- (void)textViewDidBeginEditing:(UITextView *)textView {
    CGPoint scrollPoint = textView.superview.frame.origin;

    [scroller setContentOffset:scrollPoint animated:YES];
}

@end
