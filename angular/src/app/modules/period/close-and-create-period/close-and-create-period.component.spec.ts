import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CloseAndCreatePeriodComponent } from './close-and-create-period.component';

describe('CloseAndCreatePeriodComponent', () => {
  let component: CloseAndCreatePeriodComponent;
  let fixture: ComponentFixture<CloseAndCreatePeriodComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CloseAndCreatePeriodComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CloseAndCreatePeriodComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
