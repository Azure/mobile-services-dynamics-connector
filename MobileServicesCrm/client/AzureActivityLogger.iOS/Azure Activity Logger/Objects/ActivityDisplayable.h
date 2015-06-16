//
//  ActivityDisplayable.h
//  Azure Activity Logger
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

/**
 * Simple protocol to generalize the display of an Activity.
 */

@protocol ActivityDisplayable <NSObject>

@property (nonatomic, strong) NSString *subject;
@property (nonatomic, strong) NSString *detail;
@property (nonatomic, strong) NSDate *actualEnd;
@property (nonatomic, strong) NSString *regardingObjectId;

- (UIImage *)activityImage;

@end
