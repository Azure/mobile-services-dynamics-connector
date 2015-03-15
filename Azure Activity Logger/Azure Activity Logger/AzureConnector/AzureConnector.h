#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import <WindowsAzureMobileServices/WindowsAzureMobileServices.h>

extern NSString *const AzureConnectorSyncStarted;
extern NSString *const AzureConnectorSyncCompleted;

extern NSString *const AzureConnectorSyncSuccessKey;
extern NSString *const AzureConnectorSyncFailedMessagesKey;

@interface AzureConnector : NSObject

@property (nonatomic, strong) NSString *applicationURL;
@property (nonatomic, strong) NSString *resourceURI;
@property (nonatomic, strong) NSString *clientID;

+ (AzureConnector *)sharedConnector;

- (NSDate *)lastSyncDate;

- (NSUInteger)pendingSyncCount;

- (BOOL)isLoggedIn;

- (void)loginWithController:(UIViewController *)controller completion:(MSClientLoginBlock)completion;

- (void)syncWithCompletion:(MSSyncBlock)completion;

- (void)logout;

- (void)insertTask:(NSDictionary *)task completion:(MSSyncItemBlock)completion;

@end
