//
//  FetchedResultsDataSource.h
//  Azure Activity Logger
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

/**
 * The `FetchedResultsDataSource` class is for abstracting out a standard
 * table view datasource from a view controller. It takes advantage of
 * the features available to the `NSFetchedResultsController` and its
 * delegate.
 */

/// The configuration block is called inside `tableView:cellForRowAtIndexPath:` and allows
/// the consumer to define how to setup the cell.
typedef UITableViewCell *(^CellConfigureBlock)(UITableView* tableView, id object, NSIndexPath *indexPath);
/// The change block is to allow the consumer to update itself whenever the
/// `NSFectedResultsController` is updated automatically. This removes the need for the
/// consumer to be the `NSFetchedResultsController`'s delegate.
typedef void (^FetchedResultsDidChangeBlock)();

@class NSFetchedResultsController;

@interface FetchedResultsDataSource : NSObject <UITableViewDataSource>

@property (nonatomic, copy) CellConfigureBlock cellConfigureBlock;
@property (nonatomic, copy) FetchedResultsDidChangeBlock resultsDidChangeBlock;

/// These methods provide simple customization for table view properties.
@property (nonatomic, copy) NSString *emptyResultsText;
@property (nonatomic, copy) NSString *sectionHeader;

- (instancetype)initWithFetchedResultsController:(NSFetchedResultsController *)controller;

- (id)itemAtIndexPath:(NSIndexPath *)indexPath;

@end
