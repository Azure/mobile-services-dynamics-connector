//
//  FetchedResultsDataSource.m
//  Azure Activity Logger
//

#import "FetchedResultsDataSource.h"
#import "ColorsAndFonts.h"
#import <CoreData/CoreData.h>

@interface FetchedResultsDataSource () <NSFetchedResultsControllerDelegate>

@property (nonatomic, strong) NSFetchedResultsController *resultsController;

@end

@implementation FetchedResultsDataSource

- (instancetype)initWithFetchedResultsController:(NSFetchedResultsController *)controller {
    self = [super init];
    if (self) {
        self.resultsController = controller;
        self.resultsController.delegate = self;

        NSError *fetchError = nil;
        if (![self.resultsController performFetch:&fetchError]) {
            NSLog(@"Error fetching: %@\n%@", fetchError.localizedDescription, fetchError.localizedFailureReason);
        }
    }
    return self;
}

- (id)itemAtIndexPath:(NSIndexPath *)indexPath {
    return [self.resultsController objectAtIndexPath:indexPath];
}

#pragma mark - UITableViewDataSource methods

- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView {
    return 1;
}

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section {
    id <NSFetchedResultsSectionInfo> sectionInfo = [self.resultsController.sections objectAtIndex:section];
    if ([sectionInfo numberOfObjects] == 0) {
        tableView.separatorColor = [UIColor clearColor];
        tableView.allowsSelection = NO;
        return 1;
    } else {
        tableView.allowsSelection = YES;
        tableView.separatorColor = [UIColor lightGrayColor];
    }
    
    return [sectionInfo numberOfObjects];
}

- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath {
    if ([self.resultsController.fetchedObjects count] == 0) {
        tableView.rowHeight = 250;
        
        static NSString *NoResultsIdentifier = @"NoResultsIdentifier";
        UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:NoResultsIdentifier];
        if (!cell) {
            cell = [[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:NoResultsIdentifier];
            UILabel *noResultsLabel = [[UILabel alloc] init];
            noResultsLabel.tag = 50;
            noResultsLabel.font = [UIFont fontWithName:@"HelveticaNeue-Light" size:28.0];
            noResultsLabel.textColor = MEDIUM_GREY;
            noResultsLabel.numberOfLines = 0;
           
            [cell.contentView addSubview:noResultsLabel];
            
            cell.separatorInset = UIEdgeInsetsMake(0, 0, 0, CGRectGetWidth(tableView.frame));
        }
        UILabel *noResultsLabel = (UILabel *)[cell viewWithTag:50];
        noResultsLabel.text = self.emptyResultsText;
        noResultsLabel.frame = CGRectMake(50, 20, CGRectGetWidth(tableView.frame) - 80.0, 0);
        
        [noResultsLabel sizeToFit];
        noResultsLabel.frame = CGRectMake(50, 20, CGRectGetWidth(noResultsLabel.frame), CGRectGetHeight(noResultsLabel.frame));
        
        return cell;
    }
    
    tableView.rowHeight = 50;
    
    if (self.cellConfigureBlock) {
        NSManagedObject *theObject = [self.resultsController objectAtIndexPath:indexPath];
        return self.cellConfigureBlock(tableView, theObject, indexPath);
    }

    UITableViewCell *cell = [[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:@"TEST"];
    return cell;
}

- (NSString *)tableView:(UITableView *)tableView titleForHeaderInSection:(NSInteger)section {
    return self.sectionHeader;
}

#pragma mark - NSFetchedResulstControllerDelegate methods

- (void)controllerDidChangeContent:(NSFetchedResultsController *)controller {
    if (self.resultsDidChangeBlock) {
        self.resultsDidChangeBlock();
    }
}

@end
