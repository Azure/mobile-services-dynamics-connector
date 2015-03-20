//
//  FetchedResultsDataSource.h
//  Azure Activity Logger
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

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
