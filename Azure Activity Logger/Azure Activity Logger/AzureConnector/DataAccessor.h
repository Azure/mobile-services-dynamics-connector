#import <Foundation/Foundation.h>

#import <CoreData/CoreData.h>

@interface DataAccessor : NSObject

+ (DataAccessor *)sharedAccessor;

- (NSFetchedResultsController *)fetchedResultsControllerForObject:(NSString *)object withPredicate:(NSPredicate *)predicate sortDescriptors:(NSArray *)descriptors;

- (NSFetchedResultsController *)contactsFetchedResultsController;

- (NSArray *)getActivitiesSatisfyingPredicate:(NSPredicate *)predicate;

@end
