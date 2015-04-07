//
//  QSFilter.h
//  chrandeiOSDotnet
//
//  Created by Christopher Anderson on 3/31/15.
//  Copyright (c) 2015 MobileServices. All rights reserved.
//

#ifndef chrandeiOSDotnet_QSFilter_h
#define chrandeiOSDotnet_QSFilter_h

#import <WindowsAzureMobileServices/WindowsAzureMobileServices.h>
#import <Foundation/Foundation.h>

@interface QSFilter : NSObject <MSFilter>

@property (nonatomic, strong)   MSClient *client;

@end

#endif
