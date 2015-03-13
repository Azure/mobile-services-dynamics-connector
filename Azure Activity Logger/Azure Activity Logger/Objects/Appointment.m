#import "Appointment.h"


@implementation Appointment

@dynamic ms_createdAt;
@dynamic ms_updatedAt;
@dynamic ms_version;
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
