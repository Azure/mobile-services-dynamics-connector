#import <Foundation/Foundation.h>

#import <CoreData/CoreData.h>

@class Contact;

@interface DataAccessor : NSObject

+ (DataAccessor *)sharedAccessor;

- (void)addContactToRecents:(Contact *)contact;

- (NSFetchedResultsController *)fetchedResultsControllerForObject:(NSString *)object withPredicate:(NSPredicate *)predicate sortDescriptors:(NSArray *)descriptors;

- (NSFetchedResultsController *)recentContactsFetchedResultsController;
- (NSFetchedResultsController *)contactsFetchedResultsController;

- (NSArray *)getActivitiesSatisfyingPredicate:(NSPredicate *)predicate;

@end
