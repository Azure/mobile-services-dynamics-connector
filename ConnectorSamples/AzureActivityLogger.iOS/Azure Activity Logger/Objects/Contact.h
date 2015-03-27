//
//  Contact.h
//  Azure Activity Logger
//

#import <Foundation/Foundation.h>
#import <CoreData/CoreData.h>
#import "ObjectDisplayable.h"

@interface Contact : NSManagedObject <ObjectDisplayable>

@property (nonatomic, strong) NSString *address1_City;
@property (nonatomic, strong) NSString *address1_Line1;
@property (nonatomic, strong) NSString *address1_PostalCode;
@property (nonatomic, strong) NSString *address1_StateOrProvince;
@property (nonatomic, strong) NSString *eMailAddress1;
@property (nonatomic, strong) NSString *firstName;
@property (nonatomic, strong) NSString *lastName;
@property (nonatomic, strong) NSString *id;
@property (nonatomic, strong) NSString *jobTitle;
@property (nonatomic, strong) NSDate *ms_createdAt;
@property (nonatomic, strong) NSDate *ms_updatedAt;
@property (nonatomic, strong) NSString *ms_version;
@property (nonatomic, strong) NSString *telephone1;

- (NSString *)addressInfo;

- (NSString *)mainPhone;

- (NSString *)mainEmail;

@end
