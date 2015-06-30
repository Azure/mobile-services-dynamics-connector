//
//  DataAccessor.m
//  Azure Activity Logger
//

#import "DataAccessor.h"
#import "CoreDataHelper.h"

@implementation DataAccessor

+ (DataAccessor *)sharedAccessor {
    static DataAccessor *sharedAccessor;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        sharedAccessor = [[DataAccessor alloc] init];
    });
    return sharedAccessor;
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
