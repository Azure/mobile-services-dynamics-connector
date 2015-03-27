//
//  Task.m
//  Azure Activity Logger
//

#import "Task.h"

@implementation Task

@dynamic ms_createdAt;
@dynamic ms_updatedAt;
@dynamic ms_version;
@dynamic id;
@dynamic subject;
@dynamic actualEnd;
@dynamic activityTypeCode;
@dynamic detail;
@dynamic regardingObjectId;

- (UIImage *)activityImage {
    return [UIImage imageNamed:@"icon-card-checkin"];
}

@end
