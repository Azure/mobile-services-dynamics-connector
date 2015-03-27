//
//  Contact.m
//  Azure Activity Logger
//

#import "Contact.h"
#import "NSString+StringFormatting.h"

@implementation Contact

@dynamic address1_City;
@dynamic address1_Line1;
@dynamic address1_PostalCode;
@dynamic address1_StateOrProvince;
@dynamic eMailAddress1;
@dynamic firstName;
@dynamic lastName;
@dynamic id;
@dynamic jobTitle;
@dynamic ms_createdAt;
@dynamic ms_updatedAt;
@dynamic ms_version;
@dynamic telephone1;

- (NSString *)resultLine1 {
    NSMutableString *fullName = [[NSMutableString alloc] init];
    if (self.firstName) {
        [fullName appendString:self.firstName];
    }

    if (self.lastName) {
        if (fullName.length > 0) {
            [fullName appendString:@" "];
        }
        [fullName appendString:self.lastName];
    }
    return [fullName copy];
}

- (NSString *)resultLine2 {
    return self.jobTitle;
}

- (NSString *)detailLine1 {
    return [self resultLine1];
}

- (NSString *)detailLine2 {
    return self.jobTitle;
}

- (NSString *)detailLine3 {
    return @"";
}

- (NSString *)addressInfo {
    if (self.address1_Line1 == nil && self.address1_City == nil && self.address1_StateOrProvince == nil && self.address1_PostalCode == nil) {
        return @"";
    }

    return [NSString stringWithFormat:@"%@\n%@ %@ %@", [NSString dashesForEmpty:self.address1_Line1], [NSString dashesForEmpty:self.address1_City], [NSString dashesForEmpty:self.address1_StateOrProvince], [NSString dashesForEmpty:self.address1_PostalCode]];
}

- (NSString *)mainPhone {
    return self.telephone1;
}

- (NSString *)mainEmail {
    return self.eMailAddress1;
}


@end
