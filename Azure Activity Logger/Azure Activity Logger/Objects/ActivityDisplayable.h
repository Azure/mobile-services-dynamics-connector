#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

@protocol ActivityDisplayable <NSObject>

@property (nonatomic, strong) NSString *subject;
@property (nonatomic, strong) NSString *detail;
@property (nonatomic, strong) NSDate *actualEnd;
@property (nonatomic, strong) NSString *regardingObjectId;

- (UIImage *)activityImage;

@end
