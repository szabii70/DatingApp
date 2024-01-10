import { Component, OnInit } from '@angular/core';
import { Photo } from 'src/app/_models/photo';
import { AdminService } from 'src/app/_services/admin.service';

@Component({
  selector: 'app-photo-management',
  templateUrl: './photo-management.component.html',
  styleUrls: ['./photo-management.component.css']
})
export class PhotoManagementComponent implements OnInit {
  photosForApproval: Photo[] = []

  constructor(private adminService: AdminService) {
  }

  ngOnInit(): void {
    this.getPhotosForApproval()  
  }

  getPhotosForApproval() {
    this.adminService.getPhotosForApproval().subscribe({
      next: photos => {
        this.photosForApproval = photos
        console.log(this.photosForApproval)
      }
    })
  }

  approvePhoto(id: number) {
    this.adminService.approvePhoto(id).subscribe({
      next: photo => {
        this.photosForApproval = this.photosForApproval.filter(p => p.id !== id)
      }
    })
  }

  rejcetPhoto(id: number) {
    this.adminService.rejectPhoto(id).subscribe({
      next: _ => {
        this.photosForApproval = this.photosForApproval.filter(p => p.id !== id)
      },
      error: error => console.log(error)
    })
  }
}
