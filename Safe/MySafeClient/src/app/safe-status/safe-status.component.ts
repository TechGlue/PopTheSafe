import { Component, OnInit } from '@angular/core';
import { SafestatusService } from './safestatus.service';

@Component({
  standalone: true,
  selector: 'app-safe-status',
  templateUrl: './safe-status.component.html',
})
export class SafeStatusComponent implements OnInit {
  constructor(private safeStatusService: SafestatusService) {}

  ngOnInit(): void {
    console.log('ng on init called');
    console.log('Safe' + this.safeStatusService.getSafeStatus());
  }
}
