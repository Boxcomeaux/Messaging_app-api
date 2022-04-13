import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { catchError, Observable, throwError } from 'rxjs';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private router: Router, private toastr: ToastrService, private accountService:AccountService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError(error => {
        if(error) {
          switch (error.status)
          {
            case 400:
              if(error.error.errors) 
              {
                const modalStateErrors = [];
                for(const key in error.error.errors) {
                  if(error.error.errors[key]){
                      modalStateErrors.push(error.error.errors[key])
                  }
                }
                throw modalStateErrors.flat();
              }else if(typeof error.error === 'object'){
                this.toastr.error(error.statusText, error.status);
              }else{
                this.toastr.error(error.error, error.status);
              }
              break;
            case 401:
              this.router.navigateByUrl('/');
              this.accountService.logout();
              break;
            case 404:
              this.router.navigateByUrl('/');
              break;
            case 500:
              const navigationExtras: NavigationExtras = {state: {error: error.error}};
              this.router.navigateByUrl('/server-error', navigationExtras)
              break;
            default:
              this.toastr.error('Something unexpected happened');
              console.log(error);
              break;
          }
        }
        return throwError(() => error);
      })
    );
  }
}
