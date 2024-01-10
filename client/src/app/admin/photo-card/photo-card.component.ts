import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Photo } from 'src/app/_models/photo';

@Component({
  selector: 'app-photo-card',
  templateUrl: './photo-card.component.html',
  styleUrls: ['./photo-card.component.css']
})
export class PhotoCardComponent {
  @Input() photo: Photo | undefined
  @Output() approval = new EventEmitter<number>()
  @Output() rejection = new EventEmitter<number>()

  onApproval() {
    this.approval.emit(this.photo?.id)
  }

  onRejection() {
    this.rejection.emit(this.photo?.id)
  }
}
