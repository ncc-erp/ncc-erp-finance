import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateRequestDetailComponent } from './create-request-detail.component';

describe('CreateRequestDetailComponent', () => {
  let component: CreateRequestDetailComponent;
  let fixture: ComponentFixture<CreateRequestDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateRequestDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateRequestDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
