//
//  ObjectDisplayable.h
//  Azure Activity Logger
//

#import <Foundation/Foundation.h>

@protocol ObjectDisplayable <NSObject>

- (NSString *)resultLine1;

- (NSString *)resultLine2;

- (NSString *)detailLine1;

- (NSString *)detailLine2;

- (NSString *)detailLine3;

@end
