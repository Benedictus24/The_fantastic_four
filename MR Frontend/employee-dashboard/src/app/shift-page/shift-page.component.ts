import { Component } from '@angular/core';

interface Shift {
  start: Date;
  end: Date | null;
}

@Component({
  selector: 'app-shift-page',
  templateUrl: './shift-page.component.html',
  styleUrls: ['./shift-page.component.css']
})
export class ShiftPageComponent {
  shifts: Shift[] = [];
  currentShift: Shift | null = null;

  startShift() {
    if (!this.currentShift) {
      this.currentShift = { start: new Date(), end: null };
    }
  }

  endShift() {
    if (this.currentShift) {
      this.currentShift.end = new Date();
      this.shifts.push(this.currentShift);
      this.currentShift = null;
    }
  }
}
