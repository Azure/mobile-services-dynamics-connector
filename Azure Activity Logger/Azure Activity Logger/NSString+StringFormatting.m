#import "NSString+StringFormatting.h"

@implementation NSString (StringFormatting)

// Helper method that keeps us from having (null) in the UI
+ (NSString *)dashesForEmpty:(NSString *)text {
    return (text != nil && text.length > 0) ? text : @"--";
}

@end
