//
//  Appointment.m
//  Azure Activity Logger
//

#import "Appointment.h"


@implementation Appointment

@dynamic ms_createdAt;
@dynamic ms_updatedAt;
@dynamic id;
@dynamic subject;
@dynamic activityTypeCode;
@dynamic actualEnd;
@dynamic detail;
@dynamic regardingObjectId;

- (UIImage *)activityImage {
    return [UIImage imageNamed:@"icon-card-appointment"];
}

@end
