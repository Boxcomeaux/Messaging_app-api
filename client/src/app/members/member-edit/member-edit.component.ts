import { Component, HostListener, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  @ViewChild('editForm') editFrom: NgForm;
  member: Member;
  user: User;
  @HostListener('window:beforeunload', ['$event']) unloadNotification($event: any) {
    if(this.editFrom.dirty){
      $event.returnValue = true;
    }
  }
  constructor(private accountService:AccountService, private memberService: MembersService, private toastr: ToastrService) { 
    this.accountService.currentUser$.pipe(take(1)).subscribe((user: User) => {this.user = user});
  }

  ngOnInit(): void {
    this.loadMember();
  }
  loadMember() {
    this.memberService.getMember(this.user.username).subscribe((member: Member) => this.member = member);
  }
  updateMember(){
      this.memberService.updateMember(this.member).subscribe((message: any) => {
        if(message.message.indexOf('success') > -1){
          this.toastr.success(message.message);
        }else{
          this.toastr.error(message.message);
        }
        this.editFrom.reset(this.member);
      })
  }

}
