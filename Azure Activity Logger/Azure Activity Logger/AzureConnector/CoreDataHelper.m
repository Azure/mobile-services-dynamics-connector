#import "CoreDataHelper.h"

NSString *const ConstDatabaseFileName = @"Azure_Activity_Logger.sqlite";

@implementation CoreDataHelper

@synthesize managedObjectModel, persistentStoreCoordinator, managedObjectContext;

+ (CoreDataHelper *)coreDataHelper {
    static CoreDataHelper *coreDataHelper = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        coreDataHelper = [[CoreDataHelper alloc] init];
    });

    return coreDataHelper;
}

+ (NSManagedObjectContext *)getContext {
    NSManagedObjectContext *ctx;
    if (![NSThread isMainThread]) {
        ctx = [[NSManagedObjectContext alloc] initWithConcurrencyType:NSPrivateQueueConcurrencyType];
        [ctx setPersistentStoreCoordinator:[CoreDataHelper coreDataHelper].persistentStoreCoordinator];
    } else {
        ctx = [CoreDataHelper coreDataHelper].managedObjectContext;
    }

    return ctx;
}

- (id)init {
    self = [super init];
    if (self) {
        NSBundle *mainBundle = [NSBundle mainBundle];
        NSURL *modelURL = [mainBundle URLForResource:@"Azure_Activity_Logger" withExtension:@"momd"];
        self.managedObjectModel = [[NSManagedObjectModel alloc] initWithContentsOfURL:modelURL];
        self.persistentStoreCoordinator = [[NSPersistentStoreCoordinator alloc] initWithManagedObjectModel:managedObjectModel];

        if (persistentStoreCoordinator != nil) {
            NSArray *paths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
            NSString *basePath = ([paths count] > 0) ? [paths objectAtIndex:0] : nil;

            NSURL *storeUrl = [NSURL fileURLWithPath:[basePath stringByAppendingPathComponent:ConstDatabaseFileName]];

            // Allow inferred migration from the original version of the application.
            NSDictionary *options = @{
                NSMigratePersistentStoresAutomaticallyOption : [NSNumber numberWithBool:YES],
                NSInferMappingModelAutomaticallyOption       : [NSNumber numberWithBool:YES],
                NSSQLitePragmasOption                        : @{ @"synchronous" : @"OFF" }
            };

            if (![persistentStoreCoordinator addPersistentStoreWithType:NSSQLiteStoreType configuration:nil URL:storeUrl options:options error:nil]) {
                NSLog(@"failed to add persistent store");
            }

            self.managedObjectContext = [[NSManagedObjectContext alloc] initWithConcurrencyType:NSPrivateQueueConcurrencyType];
            [managedObjectContext setPersistentStoreCoordinator:persistentStoreCoordinator];
            [managedObjectContext setMergePolicy:NSMergeByPropertyStoreTrumpMergePolicy];
        }
    }

    return self;
}

@end
