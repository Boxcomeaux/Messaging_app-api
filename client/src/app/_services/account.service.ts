import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseurl: string = 'https://localhost:5001/';
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();
  httpOptions = new HttpHeaders();
  
  constructor(private http: HttpClient) { }
  
  login(model: any){
    return this.http.post(`${this.baseurl}api/account/login`,model).pipe(
      map((response: User) => {
        const user = response;
        if(user){
          localStorage.setItem('user', JSON.stringify(user));
          this.currentUserSource.next(user);
        }
      })
    );
  }

  register(model: any){
    return this.http.post(`${this.baseurl}api/account/register`, model).pipe(
      map((data: User) => {
        if(data){
          localStorage.setItem('user', JSON.stringify(data));
          this.currentUserSource.next(data);
        }
      })
    );
  }
  setCurrentUser(user: User){
    this.currentUserSource.next(user);
  }

  getUsers(){
    return this.http.get(`${this.baseurl}api/users`);
  }

  logout(){
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
  }
}
