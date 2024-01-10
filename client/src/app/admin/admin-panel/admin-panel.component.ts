import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@microsoft/signalr';
import { Photo } from 'src/app/_models/photo';
import { AdminService } from 'src/app/_services/admin.service';

@Component({
  selector: 'app-admin-panel',
  templateUrl: './admin-panel.component.html',
  styleUrls: ['./admin-panel.component.css']
})
export class AdminPanelComponent implements OnInit {

  constructor() {
  }

  ngOnInit(): void {
  }

  

}
