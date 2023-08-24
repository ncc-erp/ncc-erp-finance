import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditRevenuemanagedComponent } from './create-edit-revenuemanaged.component';

describe('CreateEditRevenuemanagedComponent', () => {
  let component: CreateEditRevenuemanagedComponent;
  let fixture: ComponentFixture<CreateEditRevenuemanagedComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateEditRevenuemanagedComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditRevenuemanagedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
