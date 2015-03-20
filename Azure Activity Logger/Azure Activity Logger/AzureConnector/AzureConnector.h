//
//  AzureConnector.h
//  Azure Activity Logger
//

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

/**
 * The following methods are for a consumer to determine the
 * state of the connector.
 */
/// Returnw the date of the most recent successful sync. Will
/// return `nil` if there has been no successful sync yet.
- (NSDate *)lastSyncDate;
/// Returns the number of changes that are pending to be sent to
/// MWS.
- (NSUInteger)pendingSyncCount;
/// Returns whether or not the user is logged in.
- (BOOL)isLoggedIn;

/**
 * The following methods are used for managing creation and sync
 * of objects with MWS.
 */
/// Provides the ability to create a `Task` object based on a dictionary.
- (void)insertTask:(NSDictionary *)task completion:(MSSyncItemBlock)completion;
- (void)syncWithCompletion:(MSSyncBlock)completion;

/**
 * The following methods manage logging in and out of MWS.
 */
- (void)loginWithController:(UIViewController *)controller completion:(MSClientLoginBlock)completion;
- (void)logout;

@end
