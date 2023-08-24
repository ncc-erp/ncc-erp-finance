import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditExpenditureComponent } from './create-edit-expenditure.component';

describe('CreateEditExpenditureComponent', () => {
  let component: CreateEditExpenditureComponent;
  let fixture: ComponentFixture<CreateEditExpenditureComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateEditExpenditureComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditExpenditureComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
