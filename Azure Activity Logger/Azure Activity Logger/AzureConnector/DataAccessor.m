#import "DataAccessor.h"
#import "CoreDataHelper.h"
#import "Contact.h"

NSString * const kAzureConnectorRecentContactsKey = @"AzureConnectorRecentContactsKey";

@implementation DataAccessor

+ (DataAccessor *)sharedAccessor {
    static DataAccessor *sharedAccessor;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        sharedAccessor = [[DataAccessor alloc] init];
    });
    return sharedAccessor;
}

- (void)addContactToRecents:(Contact *)contact {
    NSMutableOrderedSet *recents = [NSMutableOrderedSet orderedSetWithArray:[self recentContacts]];
    
    if (!recents) {
        recents = [NSMutableOrderedSet orderedSet];
    }
    [recents removeObject:contact.id];
    [recents insertObject:contact.id atIndex:0];
    
    if (recents.count > 10) {
        [recents removeObjectAtIndex:10];
    }
    
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    [defaults setObject:[NSKeyedArchiver archivedDataWithRootObject:[recents copy]] forKey:kAzureConnectorRecentContactsKey];
}

- (NSArray *)recentContacts {
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    NSData *recentsData = [defaults dataForKey:kAzureConnectorRecentContactsKey];
    NSOrderedSet *recents = nil;
    if (recentsData) {
        recents = [NSKeyedUnarchiver unarchiveObjectWithData:recentsData];
    }
    if (!recents) {
        recents = [NSOrderedSet orderedSet];
    }
    return [recents array];
}

- (NSFetchedResultsController *)fetchedResultsControllerForObject:(NSString *)object withPredicate:(NSPredicate *)predicate sortDescriptors:(NSArray *)descriptors {
    NSFetchRequest *request = [NSFetchRequest fetchRequestWithEntityName:object];
    request.sortDescriptors = descriptors;
    request.predicate = predicate;

    NSFetchedResultsController *controller = [[NSFetchedResultsController alloc] initWithFetchRequest:request
                                                                                 managedObjectContext:[CoreDataHelper getContext]
                                                                                   sectionNameKeyPath:nil cacheName:nil];
    return controller;
}

- (NSFetchedResultsController *)recentContactsFetchedResultsController {
    NSArray *recents = [self recentContacts];
    return [self fetchedResultsControllerForObject:@"Contact"
                                     withPredicate:[NSPredicate predicateWithFormat:@"id IN %@", recents]
                                   sortDescriptors:@[[NSSortDescriptor sortDescriptorWithKey:@"lastName" ascending:YES]]];
}

- (NSFetchedResultsController *)contactsFetchedResultsController {
    NSFetchRequest *request = [NSFetchRequest fetchRequestWithEntityName:@"Contact"];
    request.sortDescriptors = @[ [NSSortDescriptor sortDescriptorWithKey:@"lastName" ascending:YES] ];

    NSFetchedResultsController *controller = [[NSFetchedResultsController alloc] initWithFetchRequest:request
                                                                                 managedObjectContext:[CoreDataHelper getContext] sectionNameKeyPath:nil cacheName:nil];
    return controller;
}

- (NSArray *)getActivitiesSatisfyingPredicate:(NSPredicate *)predicate {
    NSArray *activityTypes = @[ @"Task", @"PhoneCall", @"Appointment" ];

    NSMutableArray *retVal = [NSMutableArray array];
    for (NSString *typeName in activityTypes) {
        NSFetchRequest *request = [NSFetchRequest fetchRequestWithEntityName:typeName];
        request.predicate = predicate;

        NSError *fetchError = nil;
        NSArray *objects = [[CoreDataHelper getContext] executeFetchRequest:request error:&fetchError];
        if (!objects) {
            NSLog(@"Error fetching objects of type %@ : %@\n%@", typeName, fetchError.localizedDescription, fetchError.localizedFailureReason);
        }
        [retVal addObjectsFromArray:objects];
    }
    return [retVal copy];
}

@end
