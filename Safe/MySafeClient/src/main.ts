import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';
import {EMPTY} from 'rxjs';



bootstrapApplication(AppComponent, appConfig)
  .catch((err) => EMPTY);
