#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

typedef UITableViewCell *(^CellConfigureBlock)(id object, NSIndexPath *indexPath);

typedef void (^FetchedResultsDidChangeBlock)();

@class NSFetchedResultsController;

@interface FetchedResultsDataSource : NSObject <UITableViewDataSource>

@property (nonatomic, copy) CellConfigureBlock cellConfigureBlock;
@property (nonatomic, copy) FetchedResultsDidChangeBlock resultsDidChangeBlock;

- (instancetype)initWithFetchedResultsController:(NSFetchedResultsController *)controller;

- (id)itemAtIndexPath:(NSIndexPath *)indexPath;

@end
