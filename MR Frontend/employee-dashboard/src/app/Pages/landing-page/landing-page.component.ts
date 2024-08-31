import { Component } from '@angular/core';
import { SideNavComponent } from '../../Components/side-nav/side-nav.component';

@Component({
  selector: 'app-landing-page',
  standalone: true,
  imports: [SideNavComponent],
  templateUrl: './landing-page.component.html',
  styleUrl: './landing-page.component.css'
})
export class LandingPageComponent {

}
