import { HttpErrorResponse, HttpResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  constructor(public accountService: AccountService) { }

  model: any = {};
  
  ngOnInit(): void {

  }

  login(){
      this.accountService.login(this.model).subscribe({
        next: (response: any) => {

        },
        error: (error: HttpErrorResponse) => {
          alert(error.message);
        },
        complete: () => {

        }
       });
  }

  logout(){
    this.accountService.logout();
  }
}
