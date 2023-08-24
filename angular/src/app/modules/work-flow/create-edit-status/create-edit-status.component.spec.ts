import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditStatusComponent } from './create-edit-status.component';

describe('CreateEditStatusComponent', () => {
  let component: CreateEditStatusComponent;
  let fixture: ComponentFixture<CreateEditStatusComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateEditStatusComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditStatusComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
