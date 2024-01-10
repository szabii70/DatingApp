import { Component, OnInit } from '@angular/core';
import { AccountService } from './_services/account.service';
import { User } from './_models/user';
import { MembersService } from './_services/members.service';
import { UserParams } from './_models/userParams';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit{
  title = 'Dating app';
  users: any; 

  constructor(private accountService: AccountService,
              private memberService: MembersService) {

  }

  ngOnInit(): void {
    this.setCurrentUser()
  }

  setCurrentUser(){
    const userString = localStorage.getItem('user')
    if(!userString) return
    const user: User = JSON.parse(userString)
    this.accountService.setCurrentUser(user)
  }
}
