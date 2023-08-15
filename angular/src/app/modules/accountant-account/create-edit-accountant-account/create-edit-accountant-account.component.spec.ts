import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditAccountantAccountComponent } from './create-edit-accountant-account.component';

describe('CreateEditAccountantAccountComponent', () => {
  let component: CreateEditAccountantAccountComponent;
  let fixture: ComponentFixture<CreateEditAccountantAccountComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateEditAccountantAccountComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditAccountantAccountComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
