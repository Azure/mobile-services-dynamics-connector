//
//  CoreDataHelper.h
//  Azure Activity Logger
//

#import <Foundation/Foundation.h>
#import <CoreData/CoreData.h>

/**
 * This class is intended just to separate the Core Data stack
 * from the application's delegate.
 */

@interface CoreDataHelper : NSObject

@property (strong) NSManagedObjectModel *managedObjectModel;
@property (strong) NSPersistentStoreCoordinator *persistentStoreCoordinator;
@property (strong) NSManagedObjectContext *managedObjectContext;

+ (CoreDataHelper *)coreDataHelper;

+ (NSManagedObjectContext *)getContext;

@end
