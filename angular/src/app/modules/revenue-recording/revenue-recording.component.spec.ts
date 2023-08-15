import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RevenueRecordingComponent } from './revenue-recording.component';

describe('RevenueRecordingComponent', () => {
  let component: RevenueRecordingComponent;
  let fixture: ComponentFixture<RevenueRecordingComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RevenueRecordingComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RevenueRecordingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
