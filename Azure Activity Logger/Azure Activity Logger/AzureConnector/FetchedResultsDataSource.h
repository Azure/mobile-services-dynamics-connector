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

typedef UITableViewCell *(^CellConfigureBlock)(id object, NSIndexPath *indexPath);
typedef void (^FetchedResultsDidChangeBlock)();

@class NSFetchedResultsController;

@interface FetchedResultsDataSource : NSObject <UITableViewDataSource>

@property (nonatomic, copy) CellConfigureBlock cellConfigureBlock;
@property (nonatomic, copy) FetchedResultsDidChangeBlock resultsDidChangeBlock;

@property (nonatomic, copy) NSString *emptyResultsText;
@property (nonatomic, copy) NSString *sectionHeader;

- (instancetype)initWithFetchedResultsController:(NSFetchedResultsController *)controller;

- (id)itemAtIndexPath:(NSIndexPath *)indexPath;

@end
