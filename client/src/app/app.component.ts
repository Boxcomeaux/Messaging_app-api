import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit{
  title = 'The Dating App';
  url = 'https://localhost:5001/';
  users: any;

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
      this.http.get(`${this.url}api/users`).subscribe((data:any) => {
        this.users = data;
      }, ((error: HttpErrorResponse) => {
          alert(error.message);
      }), () => {
        
      });
  }

}
