//
//  QSFilter.m
//  chrandeiOSDotnet
//
//  Created by Christopher Anderson on 3/31/15.
//  Copyright (c) 2015 MobileServices. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <WindowsAzureMobileServices/WindowsAzureMobileServices.h>
#import "QSFilter.h"
#import "AzureConnector.h"

// This is the MSFilter protocol implementation taken from the iOS Mobile Services'\
// quickstart and modified to log the user in if a 401 Unauthorized response is received

#pragma mark * MSFilter methods

@implementation QSFilter

- (void) handleRequest:(NSURLRequest *)request
                  next:(MSFilterNextBlock)onNext
            response:(MSFilterResponseBlock)onResponse
{
    onNext(request, ^(NSHTTPURLResponse *response, NSData *data, NSError *error){
        [self filterResponse:response
                     forData:data
                   withError:error
                  forRequest:request
                      onNext:onNext
                  onResponse:onResponse];
    });
}

- (void) filterResponse: (NSHTTPURLResponse *) response
                forData: (NSData *) data
              withError: (NSError *) error
             forRequest:(NSURLRequest *) request
                 onNext:(MSFilterNextBlock) onNext
             onResponse: (MSFilterResponseBlock) onResponse
{
    //401 HTTP Status code == Unauthorized
    //When we get this message, let's try to reautheticate
    if (response.statusCode == 401) {
        // do login
        [[AzureConnector sharedConnector] loginWithController:[[[[UIApplication sharedApplication] delegate] window] rootViewController] completion:^(MSUser *user, NSError *error) {
            // error code -9001 ==user cancelled authentication - return the original response
            // if we get an error, return response as-is.
            if (error && error.code == -9001) {
                onResponse(response, data, error);
                return;
            }
            //
            NSMutableURLRequest *newRequest = [request mutableCopy];
            [newRequest setValue:self.client.currentUser.mobileServiceAuthenticationToken forHTTPHeaderField:@"X-ZUMO-AUTH"];
            onNext(newRequest, ^(NSHTTPURLResponse *innerResponse, NSData *innerData, NSError *innerError){
                [self filterResponse:innerResponse
                             forData:innerData
                           withError:innerError
                          forRequest:request
                              onNext:onNext
                          onResponse:onResponse];
            });
        }];
    }
    else {
        onResponse(response, data, error);
    }
}

@end