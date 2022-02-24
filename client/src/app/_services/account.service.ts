import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseurl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();
  httpOptions = new HttpHeaders();
  
  constructor(private http: HttpClient) { }
  
  login(model: any){
    return this.http.post(`${this.baseurl}account/login`,model).pipe(
      map((response: User) => {
        const user = response;
        if(user.username){
          localStorage.setItem('user', JSON.stringify(user));
          this.currentUserSource.next(user);
        }
      })
    );
  }

  register(model: any){
    return this.http.post(`${this.baseurl}account/register`, model).pipe(
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
    return this.http.get(`${this.baseurl}users`);
  }

  logout(){
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
  }
}
