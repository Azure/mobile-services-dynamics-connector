//
//  DataAccessor.h
//  Azure Activity Logger
//

#import <Foundation/Foundation.h>

#import <CoreData/CoreData.h>

@class Contact;

@interface DataAccessor : NSObject

+ (DataAccessor *)sharedAccessor;

- (void)addContactToRecents:(Contact *)contact;

/**
 * The following methods are intended to simplify the creation of
 * `NSFetchedResultsController` objects for use with a
 * `FetchedResultsDataSource` object.
 */
- (NSFetchedResultsController *)fetchedResultsControllerForObject:(NSString *)object withPredicate:(NSPredicate *)predicate sortDescriptors:(NSArray *)descriptors;
/// Convenience method for getting recent contacts.
- (NSFetchedResultsController *)recentContactsFetchedResultsController;
/// Convenience method for getting all contacts.
- (NSFetchedResultsController *)contactsFetchedResultsController;

- (NSArray *)getActivitiesSatisfyingPredicate:(NSPredicate *)predicate;

@end
