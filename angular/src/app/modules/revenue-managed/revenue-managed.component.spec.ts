import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RevenueManagedComponent } from './revenue-managed.component';

describe('RevenueManagedComponent', () => {
  let component: RevenueManagedComponent;
  let fixture: ComponentFixture<RevenueManagedComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RevenueManagedComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RevenueManagedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
