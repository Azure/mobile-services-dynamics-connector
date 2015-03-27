//
//  DataAccessor.h
//  Azure Activity Logger
//

#import <Foundation/Foundation.h>

#import <CoreData/CoreData.h>

@interface DataAccessor : NSObject

+ (DataAccessor *)sharedAccessor;

/**
 * The following methods are intended to simplify the creation of
 * `NSFetchedResultsController` objects for use with a
 * `FetchedResultsDataSource` object.
 */
- (NSFetchedResultsController *)fetchedResultsControllerForObject:(NSString *)object withPredicate:(NSPredicate *)predicate sortDescriptors:(NSArray *)descriptors;
/// Convenience method for getting all contacts.
- (NSFetchedResultsController *)contactsFetchedResultsController;

- (NSArray *)getActivitiesSatisfyingPredicate:(NSPredicate *)predicate;

@end
