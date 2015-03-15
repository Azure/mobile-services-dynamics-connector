#import "AzureConnector.h"
#import "CoreDataHelper.h"
#import "ADAuthenticationContext.h"
#import "SSKeychain.h"

NSString *const AzureConnectorSyncStarted = @"AzureConnectorSyncStarted";
NSString *const AzureConnectorSyncCompleted = @"AzureConnectorSyncCompleted";

/// Private key for use with NSUserDefaults to store the last successful sync date
NSString *const kAzureConnectorSyncCompletedDateKey = @"AzureConnectorSyncCompletedDateKey";

/// Private keys for use with NSUserDefaults to store application strings
NSString *const kAzureConnectorAuthorityKey = @"AzureConnectorAuthorityKey";
NSString *const kDefaultAzureAuthority = @"https://login.windows.net/sonomap.onmicrosoft.com";
NSString *const kAzureConnectorApplicationURLKey = @"AzureConnectorApplicationURLKey";
NSString *const kDefaultAzureConnectorApplicationURL = @"https://sonoma-azure-demo.azure-mobile.net/";
NSString *const kAzureConnectorResourceURIKey = @"AzureConnectorResourceURIKey";
NSString *const kDefaultAzureConnectorResourceURI = @"https://sonoma-azure-demo.azure-mobile.net/login/aad";
NSString *const kAzureConnectorClientIDKey = @"AzureConnectorClientIDKey";
NSString *const kDefaultAzureConnectorClientID = @"23bff0e9-7ce7-433a-9d4f-4fb93098c0d3";

NSString *const AzureConnectorSyncSuccessKey = @"AzureConnectorSyncSuccessKey";
NSString *const AzureConnectorSyncFailedMessagesKey = @"AzureConnectorSyncFailedMessagesKey";

@interface AzureConnector ()

@property (nonatomic, strong) NSString *authority;

@property (nonatomic, strong) MSClient *client;
@property (nonatomic, strong) NSArray *syncTables;

@end

@implementation AzureConnector

+ (AzureConnector *)sharedConnector {
    static AzureConnector *connector;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        connector = [[AzureConnector alloc] init];
    });
    return connector;
}

- (instancetype)init {
    self = [super init];
    if (self) {
        self.client = [self setupClient];
        [self loadAuthInfo];

        self.syncTables = @[
            [self.client syncTableWithName:@"Contact"],
            [self.client syncTableWithName:@"Task"],
            [self.client syncTableWithName:@"PhoneCall"],
            [self.client syncTableWithName:@"Appointment"]
        ];
    }
    return self;
}

- (MSClient *)setupClient {
    MSClient *client = [MSClient clientWithApplicationURLString:self.applicationURL applicationKey:@"UzOuYZAOxIRZzDdHYqeMGifuSqGwYu15"];
    MSCoreDataStore *dataStore = [[MSCoreDataStore alloc] initWithManagedObjectContext:[CoreDataHelper getContext]];
    client.syncContext = [[MSSyncContext alloc] initWithDelegate:nil dataSource:dataStore callback:nil];

    return client;
}

- (NSDate *)lastSyncDate {
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    return [defaults valueForKey:kAzureConnectorSyncCompletedDateKey];
}

- (void)setLastSyncDate:(NSDate *)date {
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    [defaults setValue:date forKey:kAzureConnectorSyncCompletedDateKey];
    [defaults synchronize];
}

- (NSUInteger)pendingSyncCount {
    return self.client.syncContext.pendingOperationsCount;
}

- (BOOL)isLoggedIn {
    return self.client.currentUser != nil;
}

- (void)saveAuthInfo {
    [SSKeychain setPassword:self.client.currentUser.mobileServiceAuthenticationToken forService:@"AzureMobileServiceTutorial" account:self.client.currentUser.userId];
}

- (void)loadAuthInfo {
    NSString *userid = [[SSKeychain accountsForService:@"AzureMobileServiceTutorial"][0] valueForKey:@"acct"];
    if (userid) {
        NSLog(@"userid: %@", userid);
        self.client.currentUser = [[MSUser alloc] initWithUserId:userid];
        self.client.currentUser.mobileServiceAuthenticationToken = [SSKeychain passwordForService:@"AzureMobileServiceTutorial" account:userid];

        MSTable *contactTestTable = [self.client tableWithName:@"Contact"];
        [contactTestTable readWithQueryString:@"$top=1" completion:^(MSQueryResult *result, NSError *error) {
            if (error) {
                [self clearAuthInfo];
                [self loginWithController:[[[UIApplication sharedApplication] keyWindow] rootViewController] completion:^(MSUser *user, NSError *error) {}];
            }
            NSLog(@"error : %@", error);
        }];
    }
}

- (void)clearAuthInfo {
    NSString *userId = [[SSKeychain accountsForService:@"AzureMobileServiceTutorial"][0] valueForKey:@"acct"];
    [SSKeychain deletePasswordForService:@"AzureMobileServiceTutorial" account:userId];
    self.client.currentUser.mobileServiceAuthenticationToken = nil;
}

- (ADAuthenticationContext *)authenticationContext {
    return [ADAuthenticationContext authenticationContextWithAuthority:self.authority validateAuthority:NO error:nil];
}

- (void)loginWithController:(UIViewController *)controller completion:(MSClientLoginBlock)completion {
    if (!self.client) {
        self.client = [self setupClient];
    }

    if (self.client.currentUser && self.client.currentUser.mobileServiceAuthenticationToken) {
        completion(self.client.currentUser, nil);
        return;
    }

    NSString *resourceURI = self.resourceURI;
    NSString *clientID = self.clientID;

    ADAuthenticationContext *context = [self authenticationContext];
    if (!context) {
        NSLog(@"Error getting authentication context");
        return;
    }

    [context acquireTokenWithResource:resourceURI clientId:clientID redirectUri:[NSURL URLWithString:@"ms-app://s-1-15-2-2478766528-319279558-2094806392-3380066906-3630131337-54439661-3135774793"] completionBlock:^(ADAuthenticationResult *result) {
        if (result.status != AD_SUCCEEDED) {
            NSLog(@"Error authenticating: %@\n%@", result.error.localizedDescription, result.error.localizedFailureReason);
            return;
        }

        NSDictionary *tokenDict = @{ @"access_token" : result.accessToken };

        void (^finalCompletion)(MSUser *, NSError *) = ^void(MSUser *user, NSError *error) {
            if (!error) {
                [self saveAuthInfo];
            }

            completion(user, error);
        };

        [self.client loginWithProvider:@"windowsazureactivedirectory" token:tokenDict completion:finalCompletion];
    }];
}

- (void)syncWithCompletion:(MSSyncBlock)completion {
    void (^finalCompletion)(NSError *) = ^void(NSError *error) {
        if (error) {
            NSHTTPURLResponse *response = error.userInfo[MSErrorResponseKey];

            // If the auth token is no longer valid, try reauthenticating and then
            // syncing again.
            if (response && response.statusCode == 401) {
                // we need to try authing again
                [self clearAuthInfo];
                [self loginWithController:nil completion:^(MSUser *user, NSError *error) {
                    [self syncWithCompletion:completion];
                }];
                return;
            }
        }

        completion(error);

        if (!error) {
            [self setLastSyncDate:[NSDate date]];
        }

        NSMutableDictionary *userInfo = [NSMutableDictionary dictionary];
        userInfo[AzureConnectorSyncSuccessKey] = @(!error);
        if (error) {
            userInfo[AzureConnectorSyncFailedMessagesKey] = error.localizedDescription;
        }

        [[NSNotificationCenter defaultCenter] postNotificationName:AzureConnectorSyncCompleted object:nil userInfo:userInfo];
    };

    if (self.client.currentUser == nil) {
        [self loginWithController:[[[UIApplication sharedApplication] keyWindow] rootViewController] completion:^(MSUser *user, NSError *error) {
            [self syncWithCompletion:completion];
        }];
        return;
    }

    [[NSNotificationCenter defaultCenter] postNotificationName:AzureConnectorSyncStarted object:nil];

    [self syncTables:self.syncTables WithTotalCompletion:finalCompletion];
}

- (void)syncTables:(NSArray *)tables WithTotalCompletion:(MSSyncBlock)completion {
    if (tables.count == 0) {
        completion(nil);
        return;
    }

    MSSyncTable *curTable = [tables firstObject];
    MSQuery *curQuery = [curTable query];
    [curTable pullWithQuery:curQuery queryId:nil completion:^(NSError *error) {
        if (error) {
            completion(error);
            return;
        }

        NSMutableArray *mutableTables = [NSMutableArray arrayWithArray:tables];
        [mutableTables removeObjectAtIndex:0];
        [self syncTables:[mutableTables copy] WithTotalCompletion:completion];
    }];
}

- (void)readTable:(MSSyncTable *)table {
    [table readWithCompletion:^(MSQueryResult *result, NSError *error) {
        NSLog(@"%@", result);
        NSLog(@"%@", result.items);
    }];
}

- (void)logout {
    [self clearAuthInfo];

    [self.client logout];
    self.client = nil;
    self.lastSyncDate = nil;
}

#pragma mark - Insert methods

- (void)insertTask:(NSDictionary *)task completion:(MSSyncItemBlock)completion {
    MSSyncTable *taskTable = [self.syncTables filteredArrayUsingPredicate:[NSPredicate predicateWithFormat:@"name LIKE[cd] %@", @"Task"]].firstObject;
    [taskTable insert:task completion:completion];
}

#pragma mark - Instance property accessor/setters

- (NSString *)authority {
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    NSString *authority = [defaults valueForKey:kAzureConnectorAuthorityKey];
    if (!authority) {
        authority = kDefaultAzureAuthority;
        self.authority = authority;
    }
    return authority;
}

- (void)setAuthority:(NSString *)authority {
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    [defaults setValue:authority forKey:kAzureConnectorAuthorityKey];
    [defaults synchronize];
}

- (NSString *)applicationURL {
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    NSString *url = [defaults valueForKey:kAzureConnectorApplicationURLKey];
    if (!url) {
        url = kDefaultAzureConnectorApplicationURL;
        self.applicationURL = url;
    }
    return url;
}

- (void)setApplicationURL:(NSString *)applicationURL {
    // We need to reset the client so that the next attempt to use it will cause
    // a new log in.
    //    self.client = nil;

    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    [defaults setValue:applicationURL forKey:kAzureConnectorApplicationURLKey];
    [defaults synchronize];
}

- (NSString *)resourceURI {
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    NSString *uri = [defaults valueForKey:kAzureConnectorResourceURIKey];
    if (!uri) {
        uri = kDefaultAzureConnectorResourceURI;
        self.resourceURI = uri;
    }
    return uri;
}

- (void)setResourceURI:(NSString *)resourceURI {
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    [defaults setValue:resourceURI forKey:kAzureConnectorResourceURIKey];
    [defaults synchronize];
}

- (NSString *)clientID {
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    NSString *cID = [defaults valueForKey:kAzureConnectorClientIDKey];
    if (!cID) {
        cID = kDefaultAzureConnectorClientID;
        self.clientID = cID;
    }
    return cID;
}

- (void)setClientID:(NSString *)clientID {
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    [defaults setValue:clientID forKey:kAzureConnectorClientIDKey];
    [defaults synchronize];
}

@end
