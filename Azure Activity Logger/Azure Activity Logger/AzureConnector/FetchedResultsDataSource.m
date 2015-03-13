#import "FetchedResultsDataSource.h"
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
    return [sectionInfo numberOfObjects];
}

- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath {
    if (self.cellConfigureBlock) {
        NSManagedObject *theObject = [self.resultsController objectAtIndexPath:indexPath];
        return self.cellConfigureBlock(theObject, indexPath);
    }

    UITableViewCell *cell = [[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:@"TEST"];
    return cell;
}

#pragma mark - NSFetchedResulstControllerDelegate methods

- (void)controllerDidChangeContent:(NSFetchedResultsController *)controller {
    if (self.resultsDidChangeBlock) {
        self.resultsDidChangeBlock();
    }
}

@end
