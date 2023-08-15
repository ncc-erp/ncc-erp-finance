import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditAccountTypeComponent } from './create-edit-account-type.component';

describe('CreateEditAccountTypeComponent', () => {
  let component: CreateEditAccountTypeComponent;
  let fixture: ComponentFixture<CreateEditAccountTypeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateEditAccountTypeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditAccountTypeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
