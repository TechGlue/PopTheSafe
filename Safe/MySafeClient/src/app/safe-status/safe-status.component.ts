import { Component, Input, input, OnInit } from '@angular/core';
import { SafestatusService } from './safestatus.service';
import { SafeResponse } from '../safe-response';
import { AsyncPipe } from '@angular/common';
import { Observable } from 'rxjs';

@Component({
  imports: [AsyncPipe],
  standalone: true,
  selector: 'app-safe-status',
  templateUrl: './safe-status.component.html',
})
export class SafeStatusComponent {
  @Input() safeStatus!: string;

  safeStatus$!: Observable<SafeResponse>;

  constructor(private safeStatusService: SafestatusService) {}

  ngOnInit(): void {
    this.safeStatus$ = this.safeStatusService.getSafeStatus();
  }
}
