//
//  PhoneCall.m
//  Azure Activity Logger
//

#import "PhoneCall.h"

@implementation PhoneCall

@dynamic ms_createdAt;
@dynamic ms_updatedAt;
@dynamic id;
@dynamic subject;
@dynamic actualEnd;
@dynamic activityTypeCode;
@dynamic detail;
@dynamic regardingObjectId;

- (UIImage *)activityImage {
    return [UIImage imageNamed:@"icon-card-phone"];
}

@end
