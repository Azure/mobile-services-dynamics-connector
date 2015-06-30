//
//  QSFilter.h
//  Azure Activity Logger
//

#ifndef chrandeiOSDotnet_QSFilter_h
#define chrandeiOSDotnet_QSFilter_h

#import <WindowsAzureMobileServices/WindowsAzureMobileServices.h>
#import <Foundation/Foundation.h>

@interface QSFilter : NSObject <MSFilter>

@property (nonatomic, strong)   MSClient *client;

@end

#endif
